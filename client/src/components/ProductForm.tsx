import { useEffect, useState } from 'react';
import { createProduct, updateProduct, type Product, type ProductInput } from '../api/productsApi';
import { getCategories, type Category } from '../api/categoriesApi';

interface Props {
  product: Product | null; // null = creating a new product, otherwise editing this one
  onSaved: () => void;
  onCancel: () => void;
}

const emptyForm: ProductInput = {
  name: '',
  description: '',
  purchasePrice: 0,
  sellingPrice: 0,
  minimumInv: 0,
  categoryId: null,
};

export default function ProductForm({ product, onSaved, onCancel }: Props) {
  const [form, setForm] = useState<ProductInput>(
    product
      ? {
          name: product.name,
          description: product.description,
          purchasePrice: product.purchasePrice,
          sellingPrice: product.sellingPrice,
          minimumInv: product.minimumInv,
          categoryId: product.categoryId,
        }
      : emptyForm
  );
  const [categories, setCategories] = useState<Category[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getCategories().then(setCategories).catch(() => setCategories([]));
  }, []);

  async function handleSubmit() {
    setSaving(true);
    setError(null);
    try {
      if (product) {
        await updateProduct(product.id, form);
      } else {
        await createProduct(form);
      }
      onSaved();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Uložení se nezdařilo');
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="modal-overlay" onClick={onCancel}>
      <div className="modal-panel" onClick={(e) => e.stopPropagation()}>
        <h2>{product ? 'Upravit produkt' : 'Přidat produkt'}</h2>

        {error && <p className="error-text">{error}</p>}

        <label>
          Název
          <input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
        </label>

        <label>
          Popis
          <input
            value={form.description ?? ''}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
          />
        </label>

        <label>
          Kategorie
          <select
            value={form.categoryId ?? ''}
            onChange={(e) =>
              setForm({ ...form, categoryId: e.target.value ? Number(e.target.value) : null })
            }
          >
            <option value="">-- Bez kategorie --</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </label>

        <label>
          Nákupní cena
          <input
            type="number"
            step="0.01"
            value={form.purchasePrice}
            onChange={(e) => setForm({ ...form, purchasePrice: Number(e.target.value) })}
          />
        </label>

        <label>
          Prodejní cena
          <input
            type="number"
            step="0.01"
            value={form.sellingPrice}
            onChange={(e) => setForm({ ...form, sellingPrice: Number(e.target.value) })}
          />
        </label>

        <label>
          Minimální zásoba
          <input
            type="number"
            value={form.minimumInv}
            onChange={(e) => setForm({ ...form, minimumInv: Number(e.target.value) })}
          />
        </label>

        <div className="modal-actions">
          <button className="btn-primary" onClick={handleSubmit} disabled={saving}>
            {saving ? 'Ukládám...' : 'Uložit'}
          </button>  
          <button onClick={onCancel} disabled={saving}>Zrušit</button>
        </div>
      </div>
    </div>
  );
}