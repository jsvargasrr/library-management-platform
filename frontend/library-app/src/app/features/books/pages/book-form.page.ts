import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, forkJoin, of, switchMap } from 'rxjs';
import { ToastService } from '../../../core/notifications/toast.service';
import { AuthorsApi } from '../../authors/authors.api';
import { Author } from '../../authors/authors.models';
import { BooksApi } from '../books.api';
import { Book } from '../books.models';

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './book-form.page.html',
  styleUrl: './book-form.page.scss'
})
export class BookFormPage {
  private readonly booksApi = inject(BooksApi);
  private readonly authorsApi = inject(AuthorsApi);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly loading = signal(false);
  readonly book = signal<Book | null>(null);
  readonly authors = signal<Author[]>([]);

  readonly isEdit = computed(() => !!this.route.snapshot.paramMap.get('id'));

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(250)]],
    genre: ['', [Validators.required, Validators.maxLength(80)]],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1450), Validators.max(new Date().getFullYear() + 1)]],
    pages: [1, [Validators.required, Validators.min(1)]],
    authorId: ['', [Validators.required]]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    this.loading.set(true);
    of(id)
      .pipe(
        // Cargamos autores + (opcional) el libro en paralelo para UX rápida.
        switchMap(bookId =>
          forkJoin({
            authors: this.authorsApi.list(),
            book: bookId ? this.booksApi.get(bookId) : of(null)
          })
        ),
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: ({ authors, book }) => {
          this.authors.set(authors);
          if (book) {
            this.book.set(book);
            this.form.patchValue({
              title: book.title,
              genre: book.genre,
              year: book.year,
              pages: book.pages,
              authorId: book.authorId
            });
          }
        }
      });
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.toast.error('Revisa los campos marcados.');
      return;
    }

    const payload = {
      title: this.form.controls.title.value.trim(),
      genre: this.form.controls.genre.value.trim(),
      year: this.form.controls.year.value,
      pages: this.form.controls.pages.value,
      authorId: this.form.controls.authorId.value
    };

    const id = this.route.snapshot.paramMap.get('id');

    this.loading.set(true);
    const obs = id ? this.booksApi.update(id, payload) : this.booksApi.create(payload);
    obs.pipe(finalize(() => this.loading.set(false))).subscribe({
      next: saved => {
        this.toast.success(id ? 'Libro actualizado.' : 'Libro creado.');
        this.router.navigate(['/books', saved.id]);
      }
    });
  }
}

