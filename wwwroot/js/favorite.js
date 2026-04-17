/**
 * favorite.js — Избранное (cookie-based)
 * POST /favorites/toggle/{id}
 * GET  /favorites/count
 * Кнопки: <button class="btn-favorite" data-id="123" data-is-fav="0">
 * Счётчик в шапке: .fav-count
 */
(function () {
    'use strict';

    function updateCounters(count) {
        document.querySelectorAll('.fav-count').forEach(function (el) {
            el.textContent = count;
            el.style.display = count > 0 ? '' : 'none';
        });
    }

    async function fetchCount() {
        try {
            const res = await fetch('/favorites/count', { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            if (!res.ok) return;
            const data = await res.json();
            updateCounters(data.count);
        } catch (e) { /* ignore */ }
    }

    async function toggle(btn) {
        const id = btn.dataset.id;
        const token = document.querySelector('input[name="__RequestVerificationToken"]');

        try {
            const res = await fetch(`/favorites/toggle/${id}`, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'RequestVerificationToken': token ? token.value : ''
                }
            });

            if (!res.ok) return;
            const data = await res.json();

            btn.dataset.isFav = data.isFavorite ? '1' : '0';
            btn.classList.toggle('active', data.isFavorite);

            // Меняем иконку если есть
            const icon = btn.querySelector('.fav-icon');
            if (icon) {
                icon.textContent = data.isFavorite ? '♥' : '♡';
            }

            btn.title = data.isFavorite ? 'Убрать из избранного' : 'В избранное';

            updateCounters(data.count);
        } catch (e) {
            console.error('Favorite toggle error', e);
        }
    }

    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-favorite');
        if (!btn) return;
        e.preventDefault();
        toggle(btn);
    });

    fetchCount();
})();