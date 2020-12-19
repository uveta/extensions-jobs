// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function createJob(input) {
  const createJobUrl = "/jobs/ping";
  const jobInput = { message: input };
  jobStarted();
  startLoader();
  const createResponse = await fetch(createJobUrl, {
    method: "post",
    body: JSON.stringify(jobInput),
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
  });
  const jobUrl = createResponse.headers.get("location");
  let job = await createResponse.json();
  let jobId = `${job.header.identifier.service}.${job.header.identifier.area}.${job.header.identifier.id}`;
  updateJobId(jobId);
  let jobState = job.header.state;
  updateJobState(jobState);
  let jobFinished = false;
  let jobOutputUrl = null;
  let jobOutput = null;
  let error = null;
  do {
    await sleep(1000);
    const jobResponse = await fetch(jobUrl);
    if (jobResponse.redirected) {
      console.info("Redirected to ", jobResponse.url);
      jobFinished = true;
      jobOutputUrl = jobResponse.url;
      jobOutput = await jobResponse.json();
      updateJobState("finished");
    } else {
      job = await jobResponse.json();
      jobState = job.header.state;
      updateJobState(jobState);
      console.info("Job state ", jobState);
      if (jobState === "created" || jobState == "started") {
        jobFinished = false;
      } else {
        jobFinished = true;
        if (jobState === "error") {
          error = job.header.error.message;
        }
      }
    }
  } while (!jobFinished);
  if (jobOutput.message) {
    updateJobOutput(jobOutput.message);
  } else if (jobOutputUrl) {
    const jobOutputResponse = await fetch(jobOutputUrl);
    jobOutput = await jobOutputResponse.json();
    updateJobOutput(jobOutput.message);
  }
  if (error) {
    updateJobError(error);
  }
  stopLoader();
}

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function startLoader() {}

function stopLoader() {}

function jobStarted() {
  document.getElementById("job-id-container").style.visibility = "hidden";
  document.getElementById("job-state-container").style.visibility = "hidden";
  document.getElementById("job-output-container").style.visibility = "hidden";
}

function updateJobId(id) {
  const container = document.getElementById("job-id-container");
  const value = document.getElementById("job-id");
  container.style.visibility = "visible";
  value.innerHTML = id;
}

function updateJobState(state) {
  const container = document.getElementById("job-state-container");
  const value = document.getElementById("job-state");
  container.style.visibility = "visible";
  value.innerHTML = state;
}

function updateJobOutput(output) {
  const container = document.getElementById("job-output-container");
  const value = document.getElementById("job-output");
  container.style.visibility = "visible";
  value.innerHTML = output;
}

function updateJobError(error) {
  const container = document.getElementById("job-error-container");
  const value = document.getElementById("job-error");
  container.style.visibility = "visible";
  value.innerHTML = error;
}
