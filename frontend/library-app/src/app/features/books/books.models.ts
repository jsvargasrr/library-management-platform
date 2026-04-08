export type Book = {
  id: string;
  title: string;
  year: number;
  genre: string;
  pages: number;
  authorId: string;
  authorName: string;
};

export type BookCreateRequest = {
  title: string;
  year: number;
  genre: string;
  pages: number;
  authorId: string;
};

export type BookUpdateRequest = BookCreateRequest;

