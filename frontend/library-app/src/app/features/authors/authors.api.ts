import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map } from 'rxjs';
import { API_BASE_URL } from '../../core/api/api-base-url.token';
import { ApiResponse } from '../../core/api/api-contracts';
import { Author, AuthorCreateRequest, AuthorUpdateRequest } from './authors.models';

@Injectable({ providedIn: 'root' })
export class AuthorsApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  list() {
    return this.http
      .get<ApiResponse<Author[]>>(`${this.baseUrl}/api/authors`)
      .pipe(map(r => r.data));
  }

  get(id: string) {
    return this.http
      .get<ApiResponse<Author>>(`${this.baseUrl}/api/authors/${id}`)
      .pipe(map(r => r.data));
  }

  create(req: AuthorCreateRequest) {
    return this.http
      .post<ApiResponse<Author>>(`${this.baseUrl}/api/authors`, req)
      .pipe(map(r => r.data));
  }

  update(id: string, req: AuthorUpdateRequest) {
    return this.http
      .put<ApiResponse<Author>>(`${this.baseUrl}/api/authors/${id}`, req)
      .pipe(map(r => r.data));
  }

  delete(id: string) {
    return this.http.delete<ApiResponse<unknown>>(`${this.baseUrl}/api/authors/${id}`);
  }
}

