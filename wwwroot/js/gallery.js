/**
 * gallery.js — Галерея фото на странице авто + drag-and-drop в админке
 *
 * 1. Галерея (карточка авто):
 *    <div class="gallery" data-car-id="123">
 *      <img class="gallery-main" src="..." />
 *      <div class="gallery-thumbs">
 *        <img class="gallery-thumb" src="..." data-src="большое фото" />
 *      </div>
 *    </div>
 *
 * 2. Drag-and-drop в Admin/Cars/Edit:
 *    <div class="photo-grid" data-car-id="123"> — контейнер
 *      <div class="photo-item" data-photo-id="456" draggable="true">
 */
(function () {
    'use strict';

    /* ===== ГАЛЕРЕЯ — ПЕРЕКЛЮЧЕНИЕ ФОТО ===== */
    document.addEventListener('click', function (e) {
        const thumb = e.target.closest('.gallery-thumb');
        if (!thumb) return;

        const gallery = thumb.closest('.gallery');
        if (!gallery) return;

        const main = gallery.querySelector('.gallery-main');
        if (!main) return;

        // Меняем главное фото
        const bigSrc = thumb.dataset.src || thumb.src;
        main.src = bigSrc;

        // Активный класс
        gallery.querySelectorAll('.gallery-thumb').forEach(function (t) {
            t.classList.remove('active');
        });
        thumb.classList.add('active');
    });

    /* ===== LIGHTBOX ===== */
    let lightboxOverlay = null;

    function openLightbox(src) {
        if (!lightboxOverlay) {
            lightboxOverlay = document.createElement('div');
            lightboxOverlay.id = 'gallery-lightbox';
            lightboxOverlay.style.cssText = [
                'position:fixed', 'inset:0', 'z-index:9999',
                'background:rgba(0,0,0,.88)', 'display:flex',
                'align-items:center', 'justify-content:center', 'cursor:zoom-out'
            ].join(';');
            lightboxOverlay.innerHTML = '<img style="max-width:95vw;max-height:92vh;border-radius:8px;box-shadow:0 8px 40px rgba(0,0,0,.5)" />';
            lightboxOverlay.addEventListener('click', closeLightbox);
            document.body.appendChild(lightboxOverlay);
        }
        lightboxOverlay.querySelector('img').src = src;
        lightboxOverlay.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    }

    function closeLightbox() {
        if (lightboxOverlay) {
            lightboxOverlay.style.display = 'none';
            document.body.style.overflow = '';
        }
    }

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeLightbox();
    });

    document.addEventListener('click', function (e) {
        const main = e.target.closest('.gallery-main');
        if (!main) return;
        openLightbox(main.src);
    });

    /* ===== DRAG-AND-DROP СОРТИРОВКА ФОТ0 В АДМИНКЕ ===== */
    const photoGrid = document.querySelector('.photo-grid[data-car-id]');
    if (!photoGrid) return;

    const carId = photoGrid.dataset.carId;
    let dragSrc = null;

    function getItems() {
        return Array.from(photoGrid.querySelectorAll('.photo-item[data-photo-id]'));
    }

    photoGrid.addEventListener('dragstart', function (e) {
        dragSrc = e.target.closest('.photo-item');
        if (!dragSrc) return;
        e.dataTransfer.effectAllowed = 'move';
        dragSrc.classList.add('dragging');
    });

    photoGrid.addEventListener('dragover', function (e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';

        const target = e.target.closest('.photo-item');
        if (!target || target === dragSrc) return;

        const items = getItems();
        const srcIdx = items.indexOf(dragSrc);
        const tgtIdx = items.indexOf(target);

        if (srcIdx < tgtIdx) {
            photoGrid.insertBefore(dragSrc, target.nextSibling);
        } else {
            photoGrid.insertBefore(dragSrc, target);
        }
    });

    photoGrid.addEventListener('dragend', function () {
        if (dragSrc) dragSrc.classList.remove('dragging');
        dragSrc = null;
        saveOrder();
    });

    async function saveOrder() {
        const orderedIds = getItems().map(function (el) {
            return parseInt(el.dataset.photoId, 10);
        });

        const token = document.querySelector('input[name="__RequestVerificationToken"]');

        try {
            await fetch(`/admin/cars/${carId}/photos/reorder`, {
                method: 'PATCH',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token ? token.value : ''
                },
                body: JSON.stringify(orderedIds)
            });
        } catch (e) {
            console.error('Reorder error', e);
        }
    }
})();