document.addEventListener('DOMContentLoaded', function () {
    const codeBlocks = document.querySelectorAll('pre code');
  Array.prototype.forEach.call(codeBlocks, function (code) {
      hljs.highlightBlock(code)
  });
})
