import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'authors' },
  {
    path: 'authors',
    loadComponent: () => import('./features/authors/pages/authors-list.page').then(m => m.AuthorsListPage)
  },
  {
    path: 'authors/new',
    loadComponent: () => import('./features/authors/pages/author-form.page').then(m => m.AuthorFormPage)
  },
  {
    path: 'authors/:id',
    loadComponent: () => import('./features/authors/pages/author-form.page').then(m => m.AuthorFormPage)
  },
  {
    path: 'books',
    loadComponent: () => import('./features/books/pages/books-list.page').then(m => m.BooksListPage)
  },
  {
    path: 'books/new',
    loadComponent: () => import('./features/books/pages/book-form.page').then(m => m.BookFormPage)
  },
  {
    path: 'books/:id',
    loadComponent: () => import('./features/books/pages/book-form.page').then(m => m.BookFormPage)
  },
  { path: '**', redirectTo: 'authors' }
];
