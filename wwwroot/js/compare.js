/**
 * compare.js — Сравнение автомобилей
 * POST /compare/add/{id}   — добавить
 * POST /compare/remove/{id} — убрать
 * GET  /compare/count       — счётчик
 * Обновляет все элементы с классом .compare-count
 * Кнопки: <button class="btn-compare" data-id="123" data-in-compare="0">
 */
(function () {
    'use strict';

    function updateCounters(count) {
        document.querySelectorAll('.compare-count').forEach(function (el) {
            el.textContent = count;
            el.style.display = count > 0 ? '' : 'none';
        });
    }

    async function fetchCount() {
        try {
            const res = await fetch('/compare/count', { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            if (!res.ok) return;
            const data = await res.json();
            updateCounters(data.count);
        } catch (e) { /* ignore */ }
    }

    async function toggle(btn) {
        const id = btn.dataset.id;
        const inCompare = btn.dataset.inCompare === '1';
        const url = inCompare ? `/compare/remove/${id}` : `/compare/add/${id}`;

        // Anti-CSRF token
        const token = document.querySelector('input[name="__RequestVerificationToken"]');

        try {
            const res = await fetch(url, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'RequestVerificationToken': token ? token.value : ''
                }
            });

            if (!res.ok) return;
            const data = await res.json();

            // Переключаем состояние кнопки
            const newState = !inCompare;
            btn.dataset.inCompare = newState ? '1' : '0';
            btn.classList.toggle('active', newState);
            btn.title = newState ? 'Убрать из сравнения' : 'Добавить к сравнению';

            updateCounters(data.count);
        } catch (e) {
            console.error('Compare toggle error', e);
        }
    }

    // Делегирование на document — работает и для динамически добавленных карточек
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-compare');
        if (!btn) return;
        e.preventDefault();
        toggle(btn);
    });

    // Подсветка отличий в таблице сравнения
    function highlightDiffs() {
        const table = document.querySelector('.compare-table');
        if (!table) return;

        table.querySelectorAll('tr').forEach(function (row) {
            const cells = Array.from(row.querySelectorAll('td')).slice(1); // пропускаем первый td (название строки)
            if (cells.length < 2) return;
            const values = cells.map(function (c) { return c.textContent.trim(); });
            const allSame = values.every(function (v) { return v === values[0]; });
            if (!allSame) {
                row.classList.add('diff-row');
            }
        });
    }

    // Инит при загрузке
    fetchCount();
    highlightDiffs();
})();