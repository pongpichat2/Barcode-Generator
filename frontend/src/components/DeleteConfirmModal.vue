<template>
  <Teleport to="body">
    <div v-if="show" class="modal-overlay" @click.self="$emit('cancel')">
      <div class="modal-box">
        <div class="modal-icon">⚠️</div>
        <h3>ยืนยันการลบ</h3>
        <p>ต้องการลบรหัสสินค้า <strong>{{ productCode }}</strong> หรือไม่?</p>
        <p class="sub">การกระทำนี้ไม่สามารถยกเลิกได้</p>
        <div class="modal-actions">
          <button class="btn-cancel" @click="$emit('cancel')">ยกเลิก</button>
          <button class="btn-confirm" @click="$emit('confirm')" :disabled="loading">
            {{ loading ? 'กำลังลบ...' : 'ยืนยันลบ' }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
defineProps<{
  show: boolean
  productCode: string
  loading: boolean
}>()

defineEmits<{
  cancel: []
  confirm: []
}>()
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-box {
  background: #fff;
  border-radius: 10px;
  padding: 32px 28px;
  width: 360px;
  text-align: center;
  box-shadow: 0 8px 32px rgba(0,0,0,0.2);
}

.modal-icon {
  font-size: 40px;
  margin-bottom: 12px;
}

.modal-box h3 {
  font-size: 18px;
  margin-bottom: 10px;
  color: #333;
}

.modal-box p {
  color: #555;
  font-size: 14px;
  margin-bottom: 4px;
}

.sub {
  color: #999;
  font-size: 12px;
  margin-bottom: 20px;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: center;
}

.btn-cancel {
  background: #e0e0e0;
  color: #333;
  padding: 8px 24px;
  border-radius: 6px;
}

.btn-confirm {
  background: #c62828;
  color: white;
  padding: 8px 24px;
  border-radius: 6px;
}
</style>
