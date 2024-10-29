document.addEventListener("DOMContentLoaded", () => {
    loadProducts();
});

async function loadProducts() {
    try {
        // Fetch products from GetProducts endpoint
        const response = await fetch('/Products/GetProducts');
        if (!response.ok) throw new Error('Failed to fetch products');

        const products = await response.json();
        const tbody = document.getElementById('productTableBody');
        tbody.innerHTML = ''; // Clear existing rows in the table body

        // Loop through each product and create new row
        products.forEach(product => {
            const row = `<tr>
                            <td>${product.productId}</td>
                            <td>${product.productName} </td>
                            <td>${product.brand} </td>
                            <td>${product.categoryName} </td>
                            <td>${product.price}</td>
                            <td><img src="/img/${product.imageFileName}" width="100"></td>
                            <td>${new Date(product.dateAdded).toLocaleDateString()}</td>
                            <td>
                                <a class="btn btn-primary btn-sm" data-product-id="${product.productId}">Edit</a>
                                <a class="btn btn-danger btn-sm" data-product-id="${product.productId}">Delete</a>
                            </td>
                        </tr>`;

            tbody.innerHTML += row;

        });
    }
    catch (error) {
        console.log("Error loading products", error);
    }
}