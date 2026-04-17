/**
 * calculator.js — Кредитный калькулятор
 * Вызывает GET /api/calculator?price=&downPct=&months=
 * Обновляет #calc-monthly, #calc-total, #calc-overpay, #calc-down
 */
(function () {
    'use strict';

    const form = document.getElementById('calc-form');
    if (!form) return;

    const priceInput = document.getElementById('calc-price');
    const downInput = document.getElementById('calc-down-pct');
    const monthsInput = document.getElementById('calc-months');

    const priceVal = document.getElementById('calc-price-val');
    const downVal = document.getElementById('calc-down-pct-val');
    const monthsVal = document.getElementById('calc-months-val');

    const resultMonthly = document.getElementById('calc-monthly');
    const resultTotal = document.getElementById('calc-total');
    const resultOverpay = document.getElementById('calc-overpay');
    const resultDown = document.getElementById('calc-down-amount');

    let timer = null;

    function fmt(n) {
        return Number(n).toLocaleString('ru-RU') + ' ₸';
    }

    function updateLabels() {
        if (priceVal) priceVal.textContent = fmt(priceInput.value);
        if (downVal) downVal.textContent = downInput.value + '%';
        if (monthsVal) monthsVal.textContent = monthsInput.value + ' мес.';
    }

    async function calculate() {
        const price = priceInput.value;
        const downPct = downInput.value;
        const months = monthsInput.value;

        try {
            const res = await fetch(`/api/calculator?price=${price}&downPct=${downPct}&months=${months}`);
            if (!res.ok) return;
            const data = await res.json();

            if (resultMonthly) resultMonthly.textContent = fmt(data.monthly);
            if (resultTotal) resultTotal.textContent = fmt(data.total);
            if (resultOverpay) resultOverpay.textContent = fmt(data.overpay);
            if (resultDown) resultDown.textContent = fmt(data.downAmount);
        } catch (e) {
            console.error('Calc error', e);
        }
    }

    function onInput() {
        updateLabels();
        clearTimeout(timer);
        timer = setTimeout(calculate, 400);
    }

    [priceInput, downInput, monthsInput].forEach(function (el) {
        if (el) el.addEventListener('input', onInput);
    });

    // Первый расчёт при загрузке
    updateLabels();
    calculate();
})();