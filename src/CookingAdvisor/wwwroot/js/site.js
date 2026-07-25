// Shared UI behaviour: theme toggle, mobile navigation, flash dismissal.
// Page-specific scripts live in their own view's Scripts section.
(function () {
  "use strict";

  var THEME_KEY = "cookingadvisor.theme";

  function applyTheme(theme) {
    if (theme === "light" || theme === "dark") {
      document.documentElement.setAttribute("data-bs-theme", theme);
    } else {
      document.documentElement.removeAttribute("data-bs-theme");
    }
  }

  function currentTheme() {
    var explicit = document.documentElement.getAttribute("data-bs-theme");
    if (explicit) return explicit;
    return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
  }

  function initTheme() {
    var toggle = document.querySelector("[data-theme-toggle]");
    if (!toggle) return;

    toggle.addEventListener("click", function () {
      var next = currentTheme() === "dark" ? "light" : "dark";
      applyTheme(next);
      try {
        localStorage.setItem(THEME_KEY, next);
      } catch (e) {
        // Private browsing can reject storage; the toggle still works per page.
      }
    });
  }

  function initNav() {
    var toggle = document.querySelector("[data-nav-toggle]");
    var nav = document.getElementById("site-nav");
    if (!toggle || !nav) return;

    toggle.addEventListener("click", function () {
      var open = nav.classList.toggle("is-open");
      toggle.setAttribute("aria-expanded", open ? "true" : "false");
    });
  }

  function initFlash() {
    document.querySelectorAll("[data-flash-close]").forEach(function (button) {
      button.addEventListener("click", function () {
        var flash = button.closest(".flash");
        if (flash) flash.remove();
      });
    });
  }

  // A recipe whose ImageUrl points at a missing file falls back to the
  // shared placeholder rather than showing a broken image icon.
  function initImageFallback() {
    document.querySelectorAll("img[data-fallback]").forEach(function (img) {
      img.addEventListener(
        "error",
        function () {
          if (img.dataset.fallbackApplied) return;
          img.dataset.fallbackApplied = "1";
          img.src = img.dataset.fallback;
        },
        { once: true }
      );
    });
  }

  document.addEventListener("DOMContentLoaded", function () {
    initTheme();
    initNav();
    initFlash();
    initImageFallback();
  });
})();
