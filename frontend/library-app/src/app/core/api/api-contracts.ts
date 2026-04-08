export type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T;
  traceId: string;
};

export type ApiErrorResponse = {
  success: false;
  message: string;
  errors: string[];
  traceId: string;
};

