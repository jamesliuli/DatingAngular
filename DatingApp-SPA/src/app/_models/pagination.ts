export interface Pagination {
    currentPage: number;
    pageSize: number;
    totalPages: number;
    totalItems: number;
}

export class PaginatedResult<T> {
    result: T;
    pagination: Pagination;
}