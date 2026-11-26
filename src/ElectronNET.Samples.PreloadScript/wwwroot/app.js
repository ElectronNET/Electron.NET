// Regular page script: it only knows about `window.dotnet`, which the preload script provides.
const $ = (id) => document.getElementById(id);
const api = window.dotnet;

if (!api) {
  $("status").textContent =
    "window.dotnet is not available - the page is not running inside the Electron window with the preload script.";
  $("status").classList.add("error");
} else {
  $("status").textContent = "Preload script is active: window.dotnet is available.";
  $("status").classList.add("ok");

  $("greet-form").addEventListener("submit", (e) => {
    e.preventDefault();
    api.greet($("name").value);
  });

  api.onGreeting((text) => ($("greeting").textContent = text));

  $("runtime-info").addEventListener("click", async () => {
    const info = await api.getRuntimeInfo();
    $("runtime-info-result").textContent = JSON.stringify(info, null, 2);
  });

  $("add-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const sum = await api.add(Number($("a").value), Number($("b").value));
    $("sum").textContent = ` = ${sum}`;
  });

  api.onClock((time) => ($("clock").textContent = time));
}
