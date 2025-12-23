// =================================================================
// COMPLETE FINAL TOOLTIP & VALIDATION SCRIPT
// Fully fixed: tooltip, underline, password-confirm behavior
// =================================================================

(function () {

    console.log("🟢 site.js loaded");

    const form = document.querySelector("form");
    if (!form) return;

    const formGroups = document.querySelectorAll(".form-group");

    // =================================================================
    // 1️⃣ CLEAN EMPTY data-error ON PAGE LOAD
    // =================================================================
    formGroups.forEach(group => {
        const err = group.getAttribute("data-error");

        if (!err || err.trim() === "") {
            group.removeAttribute("data-error"); // important
        }
    });

    // =================================================================
    // 2️⃣ FIND FIRST SERVER-SIDE ERROR (AFTER POST)
    // =================================================================
    let firstErrorGroup = null;

    formGroups.forEach(group => {
        const err = group.getAttribute("data-error");
        if (!firstErrorGroup && err && err.trim() !== "") {
            firstErrorGroup = group;
        }
    });

    if (firstErrorGroup) {
        console.log("⚡ Showing tooltip (initial load):", firstErrorGroup);

        firstErrorGroup.classList.add("force-tooltip");

        const input = firstErrorGroup.querySelector(".underline-input");
        if (input) input.focus();
    }

    // =================================================================
    // 3️⃣ SUBMIT HANDLER → SHOW ONLY FIRST ERROR TOOLTIP
    // =================================================================
    form.addEventListener("submit", function (e) {

        console.log("➡ Submit triggered");

        let firstError = null;

        formGroups.forEach(group => {
            const err = group.getAttribute("data-error");
            if (!firstError && err && err.trim() !== "") {
                firstError = group;
            }
        });

        if (firstError) {
            e.preventDefault();

            console.log("🚨 Tooltip shown due to submit:", firstError);

            firstError.classList.add("force-tooltip");

            const input = firstError.querySelector(".underline-input");
            if (input) {
                input.classList.add("input-validation-error");
                input.focus();
            }
        }
    });

    // =================================================================
    // 4️⃣ MAKE CONFIRM-PASSWORD RED WHEN PASSWORD IS INVALID
    // =================================================================
    (function () {
        const passwordGroup = document.querySelector('input[name="Password"]')?.closest(".form-group");
        const confirmGroup = document.querySelector('input[name="ConfirmPassword"]')?.closest(".form-group");

        if (!passwordGroup || !confirmGroup) return;

        const pwdError = passwordGroup.getAttribute("data-error");

        // If Password has an actual validation error → Confirm also red
        if (pwdError && pwdError.trim() !== "") {
            console.log("🔴 Confirm Password marked red because Password is invalid");

            confirmGroup.classList.add("input-validation-error");

            // Add red underline (ONLY underline, no tooltip)
            confirmGroup.setAttribute("data-error", "");
        }
    })();

    // =================================================================
    // 5️⃣ REMOVE TOOLTIP + RED UNDERLINE WHEN TYPING
    // =================================================================
    document.querySelectorAll(".underline-input").forEach(input => {

        input.addEventListener("input", () => {

            const group = input.closest(".form-group");

            // remove tooltip
            group.classList.remove("force-tooltip");

            // remove error underline & data-error when typing
            if (input.value.trim() !== "") {
                input.classList.remove("input-validation-error");
                group.removeAttribute("data-error");
            }
        });
    });

})();

// ============================================================================
// TOAST FUNCTIONS (Success + Error)
// ============================================================================

function showToast(id, message) {
    const toast = document.getElementById(id);
    if (!toast) return;

    toast.querySelector(".toast-message").textContent = message;
    toast.classList.add("show");

    setTimeout(() => {
        toast.classList.remove("show");
    }, 2500);
}

document.addEventListener("DOMContentLoaded", () => {

    // Read success message
    const successMsg = document.body.dataset.success;
    if (successMsg && successMsg.trim() !== "") {
        showToast("toast-success", successMsg);
    }

    // Read error message
    const errorMsg = document.body.dataset.error;
    if (errorMsg && errorMsg.trim() !== "") {
        showToast("toast-error", errorMsg);
    }
});
// =============================================================
// FULL PAGE LOADER HANDLING (FIXED)
// =============================================================

function showLoader() {
    document.getElementById("fullpage-loader").style.display = "flex";
}

function hideLoader() {
    document.getElementById("fullpage-loader").style.display = "none";
}

// Always hide loader when page loads
window.addEventListener("load", hideLoader);

document.addEventListener("DOMContentLoaded", function () {

    const forms = document.querySelectorAll("form");

    forms.forEach(form => {

        form.addEventListener("submit", function (e) {

            // ---------------------------------------------
            // 1️⃣ If the form is INVALID on client side → hide loader
            // ---------------------------------------------
            if (!form.checkValidity()) {
                hideLoader();
                return; // stop here
            }

            // ---------------------------------------------
            // 2️⃣ If modelstate contains server-side errors → hide loader
            // ---------------------------------------------
            const hasServerError =
                document.querySelector(".form-group[data-error]");

            if (hasServerError) {
                hideLoader();
                return;
            }

            // ---------------------------------------------
            // 3️⃣ If VALID → show loader (redirect will occur)
            // ---------------------------------------------
            showLoader();
        });
    });
});
