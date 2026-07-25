// Shared UI behaviour: theme toggle, flash dismissal, image fallback.
// The nav needs no script: it scrolls horizontally below the tablet breakpoint.
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

  // Off-canvas filter drawer, used on the recipe list and the suggestion page.
  // Above 1024px the panel is a plain sidebar and the toggle is hidden, so this
  // only ever runs for the overlay case.
  function initFilterDrawer() {
    var toggle = document.querySelector("[data-filter-toggle]");
    var panel = document.getElementById("filter-panel");
    if (!toggle || !panel) return;

    var backdrop = document.querySelector("[data-filter-backdrop]");
    var closeButton = panel.querySelector("[data-filter-close]");

    var FOCUSABLE = "a[href], button, input, select, textarea";

    function focusables() {
      return Array.prototype.filter.call(
        panel.querySelectorAll(FOCUSABLE),
        function (el) { return !el.disabled && el.offsetParent !== null; }
      );
    }

    function setOpen(open) {
      panel.classList.toggle("is-open", open);
      if (backdrop) backdrop.classList.toggle("is-open", open);
      toggle.setAttribute("aria-expanded", open ? "true" : "false");
      document.body.classList.toggle("has-drawer", open);

      if (open) {
        // Reading a layout property forces the pending style change to apply.
        // Without this the panel is still visibility:hidden at focus() time and
        // a hidden element silently refuses focus. requestAnimationFrame is not
        // enough here: it can run before the new styles are committed.
        void panel.offsetWidth;

        var items = focusables();
        if (items.length) items[0].focus();
      } else {
        toggle.focus();
      }
    }

    // Keeps Tab inside the drawer; the page behind it is not inert.
    function trapTab(event) {
      if (event.key !== "Tab" || !panel.classList.contains("is-open")) return;

      var items = focusables();
      if (!items.length) return;

      var first = items[0];
      var last = items[items.length - 1];

      if (event.shiftKey && document.activeElement === first) {
        event.preventDefault();
        last.focus();
      } else if (!event.shiftKey && document.activeElement === last) {
        event.preventDefault();
        first.focus();
      }
    }

    toggle.addEventListener("click", function () {
      setOpen(!panel.classList.contains("is-open"));
    });

    if (backdrop) {
      backdrop.addEventListener("click", function () { setOpen(false); });
    }
    if (closeButton) {
      closeButton.addEventListener("click", function () { setOpen(false); });
    }

    document.addEventListener("keydown", function (event) {
      if (event.key === "Escape" && panel.classList.contains("is-open")) {
        setOpen(false);
        return;
      }
      trapTab(event);
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
    initFilterDrawer();
    initFlash();
    initImageFallback();
  });
})();
