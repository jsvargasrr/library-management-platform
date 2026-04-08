import { Injectable, signal } from '@angular/core';

export type ToastKind = 'success' | 'error' | 'info';

export type Toast = {
  kind: ToastKind;
  message: string;
};

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly toast = signal<Toast | null>(null);

  success(message: string) {
    this.toast.set({ kind: 'success', message });
    this.autoClear();
  }

  error(message: string) {
    this.toast.set({ kind: 'error', message });
    this.autoClear();
  }

  info(message: string) {
    this.toast.set({ kind: 'info', message });
    this.autoClear();
  }

  clear() {
    this.toast.set(null);
  }

  private autoClear() {
    window.setTimeout(() => this.clear(), 3500);
  }
}

