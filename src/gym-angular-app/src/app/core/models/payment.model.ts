export interface Payment {
  id: number;
  membershipId: number;
  dueDate: string;
  amount: number;
  status: string;
  paidDate: string | null;
  paymentMethod: string;
  transactionId: string | null;
}

export interface PaymentResult {
  success: boolean;
  message: string;
  membershipId: number | null;
  paymentId: number | null;
  amount: number;
  paymentMethod: string;
  startDate: string | null;
  endDate: string | null;
  planName: string | null;
}

export interface ProcessPaymentRequest {
  membershipId: number;
  paymentId: number | null;
  paymentMethod: PaymentMethod;
  transactionId: string | null;
}

export interface ProcessMultiplePaymentsRequest {
  membershipId: number;
  paymentIds: number[];
  paymentMethod: PaymentMethod;
  transactionId: string | null;
}

export enum PaymentMethod {
  Cash = 0,
  Card = 1,
  BankTransfer = 2,
  Other = 3
}