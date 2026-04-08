import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map } from 'rxjs';
import { API_BASE_URL } from '../../core/api/api-base-url.token';
import { ApiResponse } from '../../core/api/api-contracts';
import { Book, BookCreateRequest, BookUpdateRequest } from './books.models';

@Injectable({ providedIn: 'root' })
export class BooksApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  list() {
    return this.http.get<ApiResponse<Book[]>>(`${this.baseUrl}/api/books`).pipe(map(r => r.data));
  }

  get(id: string) {
    return this.http.get<ApiResponse<Book>>(`${this.baseUrl}/api/books/${id}`).pipe(map(r => r.data));
  }

  create(req: BookCreateRequest) {
    return this.http.post<ApiResponse<Book>>(`${this.baseUrl}/api/books`, req).pipe(map(r => r.data));
  }

  update(id: string, req: BookUpdateRequest) {
    return this.http.put<ApiResponse<Book>>(`${this.baseUrl}/api/books/${id}`, req).pipe(map(r => r.data));
  }

  delete(id: string) {
    return this.http.delete<ApiResponse<unknown>>(`${this.baseUrl}/api/books/${id}`);
  }
}

