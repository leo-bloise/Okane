import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from "@/components/ui/pagination";

type Props = {
    currentPage: number;
    totalPages: number;
    pageSize: number;
}

export default function AccountsPaginationHandler({ currentPage, totalPages, pageSize }: Props) {
    const indexesToShow: number[] = [];

    for (let i = 0; i < totalPages; i++) {
        indexesToShow.push(i + 1);
    }

    const hasPrevious = currentPage > 0;
    const hasNext = currentPage < (totalPages - 1);

    return <Pagination>
        <PaginationContent>
            {hasPrevious && <PaginationItem>
                <PaginationPrevious href={`/accounts?page=${currentPage - 1}&pageSize=${pageSize}`} />
            </PaginationItem>}
            {indexesToShow.map(index =>
                <PaginationItem key={index}>
                    <PaginationLink href={`/accounts?page=${index - 1}&pageSize=${pageSize}`} isActive={index === (currentPage + 1)}>{index}</PaginationLink>
                </PaginationItem>
            )}
            {hasNext && <PaginationItem>
                <PaginationNext href={`/accounts?page=${currentPage + 1}&pageSize=${pageSize}`} />
            </PaginationItem>}
        </PaginationContent>
    </Pagination>
}