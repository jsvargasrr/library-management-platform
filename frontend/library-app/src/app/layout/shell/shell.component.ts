import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ToastService } from '../../core/notifications/toast.service';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss'
})
export class ShellComponent {
  private readonly toastService = inject(ToastService);

  // Computed para mantener el template simple y desacoplado del servicio.
  readonly toast = computed(() => this.toastService.toast());

  clearToast() {
    this.toastService.clear();
  }
}

