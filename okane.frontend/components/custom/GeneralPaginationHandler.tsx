import { Pagination, PaginationContent, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from "@/components/ui/pagination";

type Props = {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    previousLinkFactory: (currentPage: number, pageSize: number) => string;
    nextLinkFactory: (currentPage: number, pageSize: number) => string;
    currentLinkFactory: (index: number, pageSize: number) => string;
}

export default function GeneralPaginationHandler({ currentPage, totalPages, pageSize, nextLinkFactory, currentLinkFactory, previousLinkFactory }: Props) {
    const indexesToShow: number[] = [];

    for (let i = 0; i < totalPages; i++) {
        indexesToShow.push(i + 1);
    }

    const hasPrevious = currentPage > 0;
    const hasNext = currentPage < (totalPages - 1);

    return <Pagination>
        <PaginationContent>
            {hasPrevious && <PaginationItem>
                <PaginationPrevious href={previousLinkFactory(currentPage, pageSize)} />
            </PaginationItem>}
            {indexesToShow.map(index =>
                <PaginationItem key={index}>
                    <PaginationLink href={currentLinkFactory(index, pageSize)} isActive={index === (currentPage + 1)}>{index}</PaginationLink>
                </PaginationItem>
            )}
            {hasNext && <PaginationItem>
                <PaginationNext href={nextLinkFactory(currentPage, pageSize)} />
            </PaginationItem>}
        </PaginationContent>
    </Pagination>
}