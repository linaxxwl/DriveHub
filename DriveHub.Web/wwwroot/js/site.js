// Показать / скрыть пароль
document.addEventListener('click', function (e) {
    var btn = e.target.closest('[data-toggle-pass]');
    if (!btn) return;
    var input = btn.parentElement.querySelector('input');
    var show = input.type === 'password';
    input.type = show ? 'text' : 'password';
    btn.textContent = show ? 'Скрыть' : 'Показать';
});

// Телефон: только цифры и ведущий «+», не больше 12 символов
document.addEventListener('input', function (e) {
    if (!e.target.matches('[data-phone]')) return;
    var v = e.target.value.replace(/[^\d+]/g, '');
    v = v.charAt(0) === '+' ? '+' + v.slice(1).replace(/\+/g, '') : v.replace(/\+/g, '');
    e.target.value = v.slice(0, 12);
});