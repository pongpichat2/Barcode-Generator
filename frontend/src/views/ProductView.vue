<template>
  <div class="page">
    <!-- Header -->
    <div class="header">
      <div class="header-left">
        <span class="logo">🏷️</span>
        <span class="title">ระบบจัดการรหัสสินค้า</span>
      </div>
      <div class="header-right">
        <span class="user">👤 {{ auth.username }}</span>
        <button class="btn-logout" @click="handleLogout">ออกจากระบบ</button>
      </div>
    </div>

    <!-- Main Content -->
    <div class="container">
      <!-- Input Bar -->
      <div class="input-bar">
        <label class="input-label">รหัสสินค้า</label>
        <input
          v-model="newCode"
          type="text"
          placeholder="xxxx-xxxx-xxxx-xxxx"
          maxlength="19"
          @input="formatInput"
          @keyup.enter="handleAdd"
          :class="{ 'input-error': inputError }"
        />
        <button class="btn-generate" @click="generateCode" title="สุ่มรหัสสินค้า">
          🎲 Generate
        </button>
        <button class="btn-add" @click="handleAdd" :disabled="store.loading">
          ADD
        </button>
      </div>

      <!-- Input Error -->
      <div v-if="inputError" class="field-error">{{ inputError }}</div>

      <!-- Table -->
      <div class="table-wrapper">
        <table>
          <thead>
            <tr>
              <th style="width:50px">ID</th>
              <th>รหัสสินค้า (16 หลัก)</th>
              <th style="width:300px">บาร์โค้ด</th>
              <th style="width:80px">ลบ</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="store.loading && store.products.length === 0">
              <td colspan="4" class="center-cell">กำลังโหลด...</td>
            </tr>
            <tr v-else-if="store.products.length === 0">
              <td colspan="4" class="center-cell empty">ยังไม่มีรหัสสินค้า</td>
            </tr>
            <tr v-for="product in store.products" :key="product.id">
              <td>{{ product.id }}</td>
              <td class="code-cell">{{ product.productCode }}</td>
              <td class="barcode-cell">
                <img
                  :src="`data:image/png;base64,${product.barcodeBase64}`"
                  :alt="product.productCode"
                  class="barcode-img"
                />
              </td>
              <td class="action-cell">
                <button class="btn-delete" @click="confirmDelete(product)">ลบ</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Delete Confirm Modal -->
    <DeleteConfirmModal
      :show="!!pendingDelete"
      :product-code="pendingDelete?.productCode ?? ''"
      :loading="deleting"
      @cancel="pendingDelete = null"
      @confirm="handleDelete"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useProductStore } from '../stores/product'
import { useAuthStore } from '../stores/auth'
import DeleteConfirmModal from '../components/DeleteConfirmModal.vue'
import type { Product } from '../types'

const store = useProductStore()
const auth = useAuthStore()
const router = useRouter()

const newCode = ref('')
const inputError = ref('')
const pendingDelete = ref<Product | null>(null)
const deleting = ref(false)

onMounted(() => store.fetchAll())

function generateCode() {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'
  const segment = () => Array.from({ length: 4 }, () => chars[Math.floor(Math.random() * chars.length)]).join('')
  newCode.value = `${segment()}-${segment()}-${segment()}-${segment()}`
  inputError.value = ''
}

function formatInput() {
  // Auto-insert dashes at positions 4,9,14
  let val = newCode.value.toUpperCase().replace(/[^A-Z0-9]/g, '')
  if (val.length > 4) val = val.slice(0, 4) + '-' + val.slice(4)
  if (val.length > 9) val = val.slice(0, 9) + '-' + val.slice(9)
  if (val.length > 14) val = val.slice(0, 14) + '-' + val.slice(14)
  if (val.length > 19) val = val.slice(0, 19)
  newCode.value = val
  inputError.value = ''
}

async function handleAdd() {
  const code = newCode.value.trim()
  if (!code) {
    inputError.value = 'กรุณากรอกรหัสสินค้า'
    return
  }
  const err = await store.addProduct(code)
  if (err) {
    inputError.value = err
  } else {
    newCode.value = ''
    inputError.value = ''
  }
}

function confirmDelete(product: Product) {
  pendingDelete.value = product
}

async function handleDelete() {
  if (!pendingDelete.value) return
  deleting.value = true
  await store.deleteProduct(pendingDelete.value.id)
  deleting.value = false
  pendingDelete.value = null
}

function handleLogout() {
  auth.logout()
  router.push('/login')
}
</script>
<style scoped>
@import "../assets/styles/product.css";
</style>
