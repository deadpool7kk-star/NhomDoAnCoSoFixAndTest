// ── Book Data ──
const books = [
    { title: "Nhà Giả Kim", author: "Paulo Coelho", price: "68.000₫", old: "89.000₫", genre: "Văn học", color: "linear-gradient(160deg,#2e7d32,#66bb6a)", rating: 5, reviews: 1240, sale: true },
    { title: "Đắc Nhân Tâm", author: "Dale Carnegie", price: "75.000₫", old: "", genre: "Kỹ năng sống", color: "linear-gradient(160deg,#1565c0,#42a5f5)", rating: 5, reviews: 980 },
    { title: "Sapiens", author: "Yuval Noah Harari", price: "115.000₫", old: "145.000₫", genre: "Lịch sử", color: "linear-gradient(160deg,#4a148c,#9c27b0)", rating: 4, reviews: 760, sale: true },
    { title: "Atomic Habits", author: "James Clear", price: "89.000₫", old: "", genre: "Kỹ năng sống", color: "linear-gradient(160deg,#bf360c,#ff7043)", rating: 5, reviews: 2100 },
    { title: "Tư Duy Nhanh & Chậm", author: "Daniel Kahneman", price: "98.000₫", old: "120.000₫", genre: "Tâm lý học", color: "linear-gradient(160deg,#006064,#00bcd4)", rating: 4, reviews: 540, sale: true },
    { title: "Hoàng Tử Bé", author: "Antoine de Saint-Exupéry", price: "42.000₫", old: "", genre: "Văn học", color: "linear-gradient(160deg,#f57f17,#ffca28)", rating: 5, reviews: 3200 },
    { title: "Zero to One", author: "Peter Thiel", price: "105.000₫", old: "", genre: "Kinh doanh", color: "linear-gradient(160deg,#212121,#616161)", rating: 4, reviews: 830 },
    { title: "Dám Bị Ghét", author: "Kishimi & Koga", price: "78.000₫", old: "95.000₫", genre: "Triết học", color: "linear-gradient(160deg,#880e4f,#f06292)", rating: 5, reviews: 1560, sale: true },
];

const newBooks = [
    { title: "Người Đua Diều", author: "Khaled Hosseini", price: "88.000₫", old: "", genre: "Văn học", color: "linear-gradient(160deg,#1b5e20,#a5d6a7)", rating: 5, reviews: 420, isNew: true },
    { title: "Mindset", author: "Carol Dweck", price: "79.000₫", old: "", genre: "Tâm lý học", color: "linear-gradient(160deg,#0d47a1,#90caf9)", rating: 4, reviews: 310, isNew: true },
    { title: "Bắt Trẻ Đồng Xanh", author: "J.D. Salinger", price: "59.000₫", old: "75.000₫", genre: "Văn học", color: "linear-gradient(160deg,#e65100,#ffb74d)", rating: 4, reviews: 280, isNew: true, sale: true },
    { title: "Think Again", author: "Adam Grant", price: "92.000₫", old: "", genre: "Kỹ năng sống", color: "linear-gradient(160deg,#311b92,#9575cd)", rating: 5, reviews: 195, isNew: true },
];

let cartCount = 0;

function renderStars(n) {
    return '★'.repeat(n) + '☆'.repeat(5 - n);
}

function createBookCard(book) {
    const div = document.createElement('div');
    div.className = 'book-card';
    div.innerHTML = `
    <div class="book-card-img-placeholder" style="background:${book.color}">
      ${book.title}
    </div>
    <div class="book-card-body">
      <div class="book-genre">${book.genre}</div>
      <div class="book-title">${book.title} ${book.isNew ? '<span class="badge-new">MỚI</span>' : ''}</div>
      <div class="book-author">${book.author}</div>
      <div class="book-rating">
        <span class="stars">${renderStars(book.rating)}</span>
        <span class="rating-count">(${book.reviews.toLocaleString()})</span>
      </div>
      <div class="book-footer">
        <div>
          <span class="book-price">${book.price}</span>
          ${book.sale ? '<span class="badge-sale">SALE</span>' : ''}
          ${book.old ? `<br><span class=\"book-price-old\">${book.old}</span>` : ''}
        </div>
        <button class="btn-add" onclick="addToCart('${book.title}', event)">
          🛒 Thêm
        </button>
      </div>
    </div>`;
    return div;
}

document.addEventListener('DOMContentLoaded', () => {
    const grid = document.getElementById('booksGrid');
    books.forEach(b => grid.appendChild(createBookCard(b)));

    const newGrid = document.getElementById('newBooksGrid');
    newBooks.forEach(b => newGrid.appendChild(createBookCard(b)));

    // Category filter
    document.querySelectorAll('.cat-chip').forEach(chip => {
        chip.addEventListener('click', e => {
            e.preventDefault();
            document.querySelectorAll('.cat-chip').forEach(c => c.classList.remove('active'));
            chip.classList.add('active');
        });
    });
});

function addToCart(title, e) {
    e.stopPropagation();
    cartCount++;
    document.getElementById('cartCount').textContent = cartCount;
    showToast(`Đã thêm "${title}" vào giỏ hàng!`);
}

let toastTimer;
function showToast(msg) {
    clearTimeout(toastTimer);
    const t = document.getElementById('toast');
    document.getElementById('toastMsg').textContent = msg;
    t.classList.add('show');
    toastTimer = setTimeout(() => t.classList.remove('show'), 3000);
}
