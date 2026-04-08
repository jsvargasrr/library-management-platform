import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ToastService } from '../../../core/notifications/toast.service';
import { Book } from '../books.models';
import { BooksApi } from '../books.api';

@Component({
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './books-list.page.html',
  styleUrl: './books-list.page.scss'
})
export class BooksListPage {
  private readonly api = inject(BooksApi);
  private readonly toast = inject(ToastService);

  readonly loading = signal(false);
  readonly books = signal<Book[]>([]);
  readonly count = computed(() => this.books().length);

  ngOnInit() {
    this.refresh();
  }

  refresh() {
    this.loading.set(true);
    this.api
      .list()
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({ next: data => this.books.set(data) });
  }

  delete(book: Book) {
    const ok = window.confirm(`¿Eliminar el libro "${book.title}"?`);
    if (!ok) return;

    this.loading.set(true);
    this.api
      .delete(book.id)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: () => {
          this.toast.success('Libro eliminado.');
          this.refresh();
        }
      });
  }
}

