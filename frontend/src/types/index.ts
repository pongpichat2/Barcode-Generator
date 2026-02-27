export interface Product {
  id: number
  productCode: string
  barcodeBase64: string
  createdAt: string
}

export interface AddProductRequest {
  productCode: string
}

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  token: string
  username: string
}
