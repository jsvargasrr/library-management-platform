import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ApiErrorResponse } from './api-contracts';
import { ToastService } from '../notifications/toast.service';

export const apiErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse) {
        // Le damos prioridad al contrato de error del backend (message + errors + traceId).
        const apiErr = err.error as Partial<ApiErrorResponse> | undefined;
        const message =
          apiErr?.message ??
          (typeof err.error === 'string' ? err.error : undefined) ??
          'Ocurrió un error al comunicar con el servidor.';

        toast.error(message);
      } else {
        toast.error('Ocurrió un error inesperado.');
      }

      return throwError(() => err);
    })
  );
};

