export type Author = {
  id: string;
  fullName: string;
  birthDate: string | null; // ISO yyyy-MM-dd
  city: string | null;
  email: string;
  booksCount: number;
};

export type AuthorCreateRequest = {
  fullName: string;
  birthDate: string | null;
  city: string | null;
  email: string;
};

export type AuthorUpdateRequest = AuthorCreateRequest;

