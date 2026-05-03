/**
 * calculator.js — Расширенный кредитный калькулятор
 *
 * Поддерживает два режима:
 * 1. Компактный виджет на странице авто (элементы с id calc-*)
 * 2. Полная страница /calculator с графиком и таблицей
 *
 * API: GET /api/calculator/schedule?price=&downPct=&months=&rate=&insurance=
 */
(function () {
    'use strict';

    /* ===================================================================
       КОМПАКТНЫЙ ВИДЖЕТ (используется на карточке авто)
       =================================================================== */
    const simpleForm = document.getElementById('calc-form');
    if (simpleForm) {
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

        updateLabels();
        calculate();
    }

    /* ===================================================================
       ПОЛНАЯ СТРАНИЦА КАЛЬКУЛЯТОРА (/calculator)
       =================================================================== */
    const fullCalc = document.getElementById('full-calculator');
    if (!fullCalc) return;

    // --- Inputs ---
    const priceInput = document.getElementById('fc-price');
    const priceRange = document.getElementById('fc-price-range');
    const downInput = document.getElementById('fc-down');
    const downRange = document.getElementById('fc-down-range');
    const rateInput = document.getElementById('fc-rate');
    const rateRange = document.getElementById('fc-rate-range');
    const insInput = document.getElementById('fc-insurance');
    const periodBtns = document.querySelectorAll('.fc-period-btn');

    // --- Result displays ---
    const resMonthly = document.getElementById('fc-monthly');
    const resLoanAmt = document.getElementById('fc-loan-amt');
    const resDownPct = document.getElementById('fc-down-pct');
    const resOverpay = document.getElementById('fc-overpay');
    const resOverpayPct = document.getElementById('fc-overpay-pct');
    const resTotal = document.getElementById('fc-total');

    // --- Tabs ---
    const tabChart = document.getElementById('fc-tab-chart');
    const tabTable = document.getElementById('fc-tab-table');
    const viewChart = document.getElementById('fc-view-chart');
    const viewTable = document.getElementById('fc-view-table');
    const schedBody = document.getElementById('fc-sched-body');

    let months = 36;
    let schedChart = null;
    let donutChart = null;
    let timer = null;

    // ---- Formatting ----
    function fmt(n) {
        return Math.round(n).toLocaleString('ru-RU') + ' ₸';
    }
    function fmtNum(n) {
        return Math.round(n).toLocaleString('ru-RU');
    }
    function fmtM(n) {
        if (n >= 1000000) return (n / 1000000).toFixed(1) + 'M';
        if (n >= 1000) return Math.round(n / 1000) + 'K';
        return Math.round(n).toString();
    }
    function parseVal(el) {
        return parseFloat(el.value.replace(/\s+/g, '').replace(',', '.')) || 0;
    }
    function setInputVal(el, n) {
        el.value = Math.round(n).toLocaleString('ru-RU');
    }

    // ---- Period buttons ----
    periodBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            periodBtns.forEach(function (b) { b.classList.remove('fc-active'); });
            btn.classList.add('fc-active');
            months = parseInt(btn.dataset.months, 10);
            requestCalculate();
        });
    });

    // ---- Sync price range <-> input ----
    if (priceRange) {
        priceRange.addEventListener('input', function () {
            setInputVal(priceInput, priceRange.value);
            syncDownFromPct();
            requestCalculate();
        });
    }
    if (priceInput) {
        priceInput.addEventListener('input', function () {
            var v = parseVal(priceInput);
            if (priceRange) priceRange.value = Math.min(+priceRange.max, Math.max(+priceRange.min, v));
            syncDownFromPct();
            requestCalculate();
        });
    }

    // ---- Down payment: range = % of price ----
    function syncDownFromPct() {
        if (!downRange || !downInput) return;
        var price = parseVal(priceInput);
        var pct = parseInt(downRange.value, 10);
        setInputVal(downInput, Math.round(price * pct / 100));
    }
    function syncPctFromDown() {
        if (!downRange || !priceInput) return;
        var price = parseVal(priceInput);
        var down = parseVal(downInput);
        var pct = price > 0 ? Math.round(down / price * 100) : 0;
        downRange.value = Math.min(80, Math.max(0, pct));
    }
    if (downRange) {
        downRange.addEventListener('input', function () {
            syncDownFromPct();
            requestCalculate();
        });
    }
    if (downInput) {
        downInput.addEventListener('input', function () {
            syncPctFromDown();
            requestCalculate();
        });
    }

    // ---- Rate ----
    if (rateRange) {
        rateRange.addEventListener('input', function () {
            rateInput.value = rateRange.value;
            requestCalculate();
        });
    }
    if (rateInput) {
        rateInput.addEventListener('input', function () {
            if (rateRange) rateRange.value = Math.min(+rateRange.max, Math.max(+rateRange.min, parseFloat(rateInput.value) || 18));
            requestCalculate();
        });
    }

    // ---- Insurance ----
    if (insInput) {
        insInput.addEventListener('input', requestCalculate);
    }

    // ---- Tab switching ----
    function showTab(tab) {
        if (viewChart) viewChart.style.display = tab === 'chart' ? '' : 'none';
        if (viewTable) viewTable.style.display = tab === 'table' ? '' : 'none';
        if (tabChart) tabChart.classList.toggle('fc-tab-active', tab === 'chart');
        if (tabTable) tabTable.classList.toggle('fc-tab-active', tab === 'table');
    }
    if (tabChart) tabChart.addEventListener('click', function () { showTab('chart'); });
    if (tabTable) tabTable.addEventListener('click', function () { showTab('table'); });

    // ---- Debounced calculate ----
    function requestCalculate() {
        clearTimeout(timer);
        timer = setTimeout(doCalculate, 350);
    }

    async function doCalculate() {
        var price = parseVal(priceInput);
        var down = parseVal(downInput);
        var rate = parseFloat(rateInput ? rateInput.value : 18) || 18;
        var insurance = parseVal(insInput || { value: '0' });
        var downPct = price > 0 ? Math.round(down / price * 100) : 0;

        if (price <= 0) return;

        try {
            var url = '/api/calculator/schedule?price=' + price
                + '&downPct=' + downPct
                + '&months=' + months
                + '&rate=' + rate
                + '&insurance=' + insurance;

            var res = await fetch(url);
            if (!res.ok) return;
            var data = await res.json();

            renderMetrics(data, down);
            renderCharts(data);
            renderTable(data);

        } catch (e) {
            console.error('Calculator error:', e);
        }
    }

    function renderMetrics(data, down) {
        if (resMonthly) resMonthly.textContent = fmt(data.monthly);
        if (resLoanAmt) resLoanAmt.textContent = fmtNum(data.principal) + ' ₸';
        if (resDownPct) resDownPct.textContent = 'взнос ' + Math.round(down / (down + data.principal) * 100) + '%';
        if (resOverpay) resOverpay.textContent = fmt(data.overpay);
        if (resOverpayPct) resOverpayPct.textContent = '+' + Math.round(data.overpay / data.principal * 100) + '%';
        if (resTotal) resTotal.textContent = fmt(data.total);
    }

    function renderCharts(data) {
        var rows = data.rows;
        var labels = rows.map(function (r) { return r.date.slice(3); }); // MM.YYYY
        var principals = rows.map(function (r) { return r.principal; });
        var interests = rows.map(function (r) { return r.interest; });
        var balances = rows.map(function (r) { return r.balance; });

        // Reduce labels for readability
        var step = Math.ceil(labels.length / 24);
        var sparseLabels = labels.map(function (l, i) { return i % step === 0 ? l : ''; });

        if (schedChart) {
            schedChart.data.labels = sparseLabels;
            schedChart.data.datasets[0].data = principals;
            schedChart.data.datasets[1].data = interests;
            schedChart.data.datasets[2].data = balances;
            schedChart.update('none');
        } else {
            var ctx = document.getElementById('fc-bar-chart');
            if (!ctx) return;
            schedChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: sparseLabels,
                    datasets: [
                        {
                            label: 'Осн. долг',
                            data: principals,
                            backgroundColor: '#1a5fb4',
                            stack: 'payments',
                            order: 2
                        },
                        {
                            label: 'Проценты',
                            data: interests,
                            backgroundColor: '#c0392b',
                            stack: 'payments',
                            order: 2
                        },
                        {
                            label: 'Остаток долга',
                            type: 'line',
                            data: balances,
                            borderColor: '#27ae60',
                            backgroundColor: 'transparent',
                            borderWidth: 2,
                            pointRadius: 0,
                            yAxisID: 'y2',
                            order: 1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false },
                        tooltip: {
                            callbacks: {
                                label: function (ctx) {
                                    return ' ' + ctx.dataset.label + ': ' + fmtNum(ctx.raw) + ' ₸';
                                }
                            }
                        }
                    },
                    scales: {
                        x: {
                            stacked: true,
                            ticks: { color: '#888', font: { size: 10 }, maxRotation: 0 },
                            grid: { display: false }
                        },
                        y: {
                            stacked: true,
                            ticks: {
                                color: '#888',
                                font: { size: 10 },
                                callback: function (v) { return fmtM(v); }
                            },
                            grid: { color: 'rgba(128,128,128,0.1)' }
                        },
                        y2: {
                            type: 'linear',
                            position: 'right',
                            ticks: {
                                color: '#27ae60',
                                font: { size: 10 },
                                callback: function (v) { return fmtM(v); }
                            },
                            grid: { display: false }
                        }
                    }
                }
            });
        }

        // Donut
        var total = data.principal + data.overpay;
        var lPct = Math.round(data.principal / total * 100);
        var oPct = 100 - lPct;

        if (donutChart) {
            donutChart.data.labels[0] = 'Тело кредита ' + lPct + '%';
            donutChart.data.labels[1] = 'Переплата ' + oPct + '%';
            donutChart.data.datasets[0].data = [data.principal, data.overpay];
            donutChart.update('none');
        } else {
            var dCtx = document.getElementById('fc-donut-chart');
            if (!dCtx) return;
            donutChart = new Chart(dCtx, {
                type: 'doughnut',
                data: {
                    labels: ['Тело кредита ' + lPct + '%', 'Переплата ' + oPct + '%'],
                    datasets: [{
                        data: [data.principal, data.overpay],
                        backgroundColor: ['#1a5fb4', '#c0392b'],
                        borderWidth: 0,
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    cutout: '65%',
                    plugins: {
                        legend: {
                            display: true,
                            position: 'right',
                            labels: { font: { size: 12 }, padding: 16, usePointStyle: true, pointStyleWidth: 10 }
                        },
                        tooltip: {
                            callbacks: {
                                label: function (ctx) { return ' ' + ctx.label + ': ' + fmt(ctx.raw); }
                            }
                        }
                    }
                }
            });
        }
    }

    function renderTable(data) {
        if (!schedBody) return;
        schedBody.innerHTML = data.rows.map(function (row, i) {
            var pct = data.principal > 0
                ? Math.max(0, Math.min(100, Math.round((1 - row.balance / data.principal) * 100)))
                : 0;
            return '<tr>'
                + '<td class="fc-td-date">' + row.date + '</td>'
                + '<td class="text-end">' + fmtNum(row.payment) + ' ₸</td>'
                + '<td class="text-end fc-td-principal">'
                + fmtNum(row.principal) + ' ₸'
                + '<div class="fc-prog-wrap"><div class="fc-prog-bar" style="width:' + pct + '%"></div></div>'
                + '</td>'
                + '<td class="text-end fc-td-interest">' + fmtNum(row.interest) + ' ₸</td>'
                + '<td class="text-end">' + fmtNum(row.balance) + ' ₸</td>'
                + '</tr>';
        }).join('');
    }

    // Initial render
    syncDownFromPct();
    doCalculate();

})();