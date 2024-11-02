document.addEventListener("DOMContentLoaded", () => {
    loadProducts();
});


async function loadProducts() {
    try {
        // Fetch products from GetProducts endpoint
        const response = await fetch('/Products/GetProducts');
        if (!response.ok) throw new Error("Failed to fetch products");

        const products = await response.json();
        const tbody = document.getElementById('productTableBody');
        tbody.innerHTML = ''; // Clear existing rows in the table

        // Loop through each product and create new row
        products.forEach(product => {
            const row = `<tr>
                        <td>${product.productId}</td>
                        <td>${product.productName}</td>
                        <td>${product.brand} </td>
                        <td>${product.categoryName}</td>
                        <td>${product.price}</td>
                        <td><img src="/img/${product.imageFileName}" width="100"></td>
                        <td>${new Date(product.dateAdded).toLocaleDateString()}</td>
                        <td>
                            <a class="btn btn-primary btn-sm" href="/Products/UpdateProduct/${product.productId}">Edit</a>
                            <a class="btn btn-primary btn-sm" onclick="deleteProduct(${product.productId})">Delete</a>
                        </td>
                     </tr>`;
            tbody.innerHTML += row;
        });
    }
    catch (error) {
        console.log("Error loading products", error);
    }
}

async function deleteProduct(productId) {
    if (!confirm('Are you sure you want to delete this product?')) {
        return;
    }

    try {
        // Get the anti-forgery token
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await fetch(`/Products/DeleteProduct/${productId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        });

        if (!response.ok) {
            throw new Error('Failed to delete product');
        }

        // Refresh the products list
        window.location.reload();
    }
    catch (error) {
        console.error('Error deleting product:', error);
        toastr.error('Failed to delete product');
    }
}