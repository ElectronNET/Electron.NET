const links = ["windows", "crashhang", "menus", "shortcuts", "shell", "notifications", "dialogs", "tray", "ipc", "hosthook", "appsysinformation", "clipboard", "pdf", "desktopcapturer", "update"];

fetch('about').then((aboutPage) => {
  aboutPage.text().then(pageContent => {
    const template = document.createRange().createContextualFragment(pageContent);
    document.querySelector('body').appendChild(template.firstElementChild.content);
  });
});

links.forEach(async pageName => {
  const page = await fetch(pageName);
  const pageContent = await page.text();
  const template = document.createRange().createContextualFragment(pageContent);
  document.querySelector('.content').appendChild(template.firstElementChild.content);
});