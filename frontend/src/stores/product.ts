import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Product } from '../types'
import api from '../services/api'

export const useProductStore = defineStore('product', () => {
  const products = ref<Product[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchAll() {
    loading.value = true
    error.value = null
    try {
      const res = await api.get<Product[]>('/products')
      products.value = res.data
    } catch (e: any) {
      error.value = e.response?.data?.message ?? 'เกิดข้อผิดพลาด'
    } finally {
      loading.value = false
    }
  }

  async function addProduct(productCode: string): Promise<string | null> {
    try {
      const res = await api.post<Product>('/products', { productCode })
      products.value.unshift(res.data)
      return null
    } catch (e: any) {
      return e.response?.data?.message ?? 'เกิดข้อผิดพลาด'
    }
  }

  async function deleteProduct(id: number): Promise<string | null> {
    try {
      await api.delete(`/products/${id}`)
      products.value = products.value.filter(p => p.id !== id)
      return null
    } catch (e: any) {
      return e.response?.data?.message ?? 'เกิดข้อผิดพลาด'
    }
  }

  return { products, loading, error, fetchAll, addProduct, deleteProduct }
})
