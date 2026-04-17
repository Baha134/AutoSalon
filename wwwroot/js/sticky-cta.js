/**
 * sticky-cta.js — Мобильная «липкая» кнопка + Drawer-фильтр
 *
 * 1. Sticky CTA скрывается, когда основная кнопка заявки видима на экране.
 *    <div class="sticky-cta" id="sticky-cta"> ... </div>
 *    <div id="lead-anchor"></div>  — якорь рядом с основной формой
 *
 * 2. Drawer-фильтр:
 *    <button class="filters-drawer-btn" id="drawer-open-btn">Фильтры</button>
 *    <div class="filters-drawer" id="filters-drawer">
 *      <div class="drawer-backdrop" id="drawer-backdrop"></div>
 *      <div class="drawer-panel">
 *        <div class="drawer-handle"></div>
 *        ... фильтры ...
 *      </div>
 *    </div>
 */
(function () {
    'use strict';

    /* ===== STICKY CTA ===== */
    var stickyCta = document.getElementById('sticky-cta');
    var leadAnchor = document.getElementById('lead-anchor');

    if (stickyCta && leadAnchor) {
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                // Скрываем sticky когда якорь виден, показываем когда не виден
                stickyCta.classList.toggle('hidden', entry.isIntersecting);
            });
        }, { threshold: 0.1 });

        observer.observe(leadAnchor);
    }

    /* ===== DRAWER ФИЛЬТР ===== */
    var openBtn = document.getElementById('drawer-open-btn');
    var drawer = document.getElementById('filters-drawer');
    var backdrop = document.getElementById('drawer-backdrop');
    var closeBtn = document.getElementById('drawer-close-btn');

    function openDrawer() {
        if (!drawer) return;
        drawer.classList.add('open');
        document.body.style.overflow = 'hidden';
    }

    function closeDrawer() {
        if (!drawer) return;
        drawer.classList.remove('open');
        document.body.style.overflow = '';
    }

    if (openBtn) openBtn.addEventListener('click', openDrawer);
    if (backdrop) backdrop.addEventListener('click', closeDrawer);
    if (closeBtn) closeBtn.addEventListener('click', closeDrawer);

    // Закрытие по Escape
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeDrawer();
    });

    /* ===== ФОРМА ЗАЯВКИ — AJAX SUBMIT ===== */
    var leadForms = document.querySelectorAll('.lead-form');

    leadForms.forEach(function (form) {
        form.addEventListener('submit', async function (e) {
            e.preventDefault();

            var btn = form.querySelector('[type="submit"]');
            if (btn) { btn.disabled = true; btn.textContent = 'Отправляем...'; }

            var data = new FormData(form);

            try {
                var res = await fetch('/leads', {
                    method: 'POST',
                    headers: { 'X-Requested-With': 'XMLHttpRequest' },
                    body: data
                });

                var json = await res.json();

                if (json.success) {
                    form.innerHTML = '<div class="lead-success"><span>✅</span><p>Заявка принята! Мы свяжемся с вами в ближайшее время.</p></div>';
                } else {
                    showFormErrors(form, json.errors);
                    resetBtn(btn);
                }
            } catch (err) {
                console.error('Lead submit error', err);
                resetBtn(btn);
            }
        });
    });

    function showFormErrors(form, errors) {
        var existing = form.querySelector('.lead-errors');
        if (existing) existing.remove();

        if (!errors || !errors.length) return;
        var div = document.createElement('div');
        div.className = 'lead-errors alert alert-danger';
        div.textContent = errors.join('. ');
        form.prepend(div);
    }

    function resetBtn(btn) {
        if (!btn) return;
        btn.disabled = false;
        btn.textContent = 'Отправить заявку';
    }

})();