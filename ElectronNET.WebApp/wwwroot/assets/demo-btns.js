const demoBtns = document.querySelectorAll('.js-container-target');

// Listen for demo button clicks
Array.prototype.forEach.call(demoBtns, function (btn) {
  btn.addEventListener('click', function (event) {
      const parent = event.target.parentElement;

    // Toggles the "is-open" class on the demo's parent element.
    parent.classList.toggle('is-open');
  })
})
