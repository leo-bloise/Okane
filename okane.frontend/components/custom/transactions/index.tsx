import { Transaction } from "@/lib/types/transaction";
import TransactionTable from "./TransactionTable";
import GeneralPaginationHandler from "../GeneralPaginationHandler";

type Props = {
    transactions: Transaction[];
    currentPage: number;
    totalPages: number;
    pageSize: number;    
}

export default function TransactionsSection({ transactions, currentPage, totalPages, pageSize }: Props) {
    return transactions.length > 0 ? <section className="flex flex-col gap-y-4">
        <TransactionTable transactions={transactions} />
        <GeneralPaginationHandler
            currentPage={currentPage}
            totalPages={totalPages}
            pageSize={pageSize}
            currentLinkFactory={(index, pageSize) => {
                return `/transactions?page=${index - 1}&pageSize=${pageSize}`;
            }}
            nextLinkFactory={(curr, pageSize) => {
                return `/transactions?page=${curr + 1}&pageSize=${pageSize}`
            }}
            previousLinkFactory={(currentPage, pageSize) => {
                return `/transactions?page=${currentPage - 1}&pageSize=${pageSize}`
            }}
        />
    </section> : <></>;
}