<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-header">
        <h2>🏷️ ระบบจัดการรหัสสินค้า</h2>
        <p>กรุณาเข้าสู่ระบบเพื่อดำเนินการต่อ</p>
      </div>

      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label>ชื่อผู้ใช้</label>
          <input
            v-model="form.username"
            type="text"
            placeholder="กรอกชื่อผู้ใช้"
            autocomplete="username"
            :disabled="loading"
          />
        </div>

        <div class="form-group">
          <label>รหัสผ่าน</label>
          <input
            v-model="form.password"
            type="password"
            placeholder="กรอกรหัสผ่าน"
            autocomplete="current-password"
            :disabled="loading"
          />
        </div>

        <div v-if="errorMsg" class="error-msg">{{ errorMsg }}</div>

        <button type="submit" class="btn-login" :disabled="loading">
          {{ loading ? 'กำลังเข้าสู่ระบบ...' : 'เข้าสู่ระบบ' }}
        </button>
      </form>

      <div class="hint">
        <small>ทดสอบ: admin / Admin@1234</small>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const router = useRouter()

const form = ref({ username: '', password: '' })
const loading = ref(false)
const errorMsg = ref('')

async function handleLogin() {
  errorMsg.value = ''
  if (!form.value.username || !form.value.password) {
    errorMsg.value = 'กรุณากรอกชื่อผู้ใช้และรหัสผ่าน'
    return
  }
  loading.value = true
  try {
    await auth.login(form.value)
    router.push('/')
  } catch (e: any) {
    errorMsg.value = e.response?.data?.message ?? 'เข้าสู่ระบบไม่สำเร็จ'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
@import "../assets/styles/login.css";
</style>
