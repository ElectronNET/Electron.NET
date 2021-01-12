document.addEventListener('DOMContentLoaded', function () {
  setTimeout(() => {
    const codeBlocks = document.querySelectorAll('pre code');
    Array.prototype.forEach.call(codeBlocks, function (code) {
      hljs.highlightBlock(code)
    });
  }, 1000);
})
