import { useEffect, useState } from 'react';
import { getProducts, deleteProduct, type Product } from '../api/productsApi';
import ProductForm from '../components/ProductForm';
import './Products.css';

export default function Products() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editing, setEditing] = useState<Product | 'new' | null>(null);

  async function loadProducts() {
    setLoading(true);
    setError(null);
    try {
      setProducts(await getProducts());
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Nepodařilo se načíst produkty');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadProducts();
  }, []);

  async function handleDelete(id: number) {
    if (!confirm('Opravdu chcete tento produkt smazat?')) return;
    try {
      await deleteProduct(id);
      await loadProducts();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Smazání se nezdařilo');
    }
  }

  function handleSaved() {
    setEditing(null);
    loadProducts();
  }

  return (
    <div className="products-page">
      <div className="products-header">
        <h1>Produkty</h1>
        <button className="btn-primary" onClick={() => setEditing('new')}>+ Přidat produkt</button>
      </div>

      {loading && <p>Načítám...</p>}
      {error && <p className="error-text">{error}</p>}

      {!loading && !error && (
        <table className="products-table">
          <thead>
            <tr>
              <th>Název</th>
              <th>Kategorie</th>
              <th>Nákupní cena</th>
              <th>Prodejní cena</th>
              <th>Sklad</th>
              <th>Min. zásoba</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {products.map((p) => (
              <tr key={p.id} className={p.warehouseInv < p.minimumInv ? 'row-low-stock' : ''}>
                <td>{p.name}</td>
                <td>{p.categoryName ?? '—'}</td>
                <td>{p.purchasePrice.toFixed(2)} Kč</td>
                <td>{p.sellingPrice.toFixed(2)} Kč</td>
                <td>{p.warehouseInv} ks</td>
                <td>{p.minimumInv} ks</td>
                <td className="row-actions">
                  <button onClick={() => setEditing(p)}>Upravit</button>
                  <button onClick={() => handleDelete(p.id)}>Smazat</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {editing !== null && (
        <ProductForm
          product={editing === 'new' ? null : editing}
          onSaved={handleSaved}
          onCancel={() => setEditing(null)}
        />
      )}
    </div>
  );
}