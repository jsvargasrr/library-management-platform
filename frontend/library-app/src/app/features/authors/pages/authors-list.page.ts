import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ToastService } from '../../../core/notifications/toast.service';
import { AuthorsApi } from '../authors.api';
import { Author } from '../authors.models';

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './authors-list.page.html',
  styleUrl: './authors-list.page.scss'
})
export class AuthorsListPage {
  private readonly api = inject(AuthorsApi);
  private readonly toast = inject(ToastService);

  readonly loading = signal(false);
  readonly authors = signal<Author[]>([]);

  readonly count = computed(() => this.authors().length);

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.loading.set(true);
    this.api
      .list()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: data => this.authors.set(data)
      });
  }

  delete(author: Author) {
    const ok = window.confirm(`¿Eliminar el autor "${author.fullName}"?`);
    if (!ok) return;

    this.loading.set(true);
    this.api
      .delete(author.id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: () => {
          this.toast.success('Autor eliminado.');
          this.refresh();
        }
      });
  }
}

