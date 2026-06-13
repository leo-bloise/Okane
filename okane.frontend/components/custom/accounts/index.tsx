import { Account } from "@/lib/types/accounts"
import AccountsTable from "./AccountsTable";
import GeneralPaginationHandler from "../GeneralPaginationHandler";

type Props = {
    accounts: Account[];
    currentPage: number;
    totalPages: number;
    pageSize: number;
}

export default function AccountsSection({ accounts, currentPage, totalPages, pageSize }: Props) {
    return accounts.length > 0 ? <section className="flex flex-col gap-y-4">
        <AccountsTable accounts={accounts} />
        <GeneralPaginationHandler
            currentPage={currentPage}
            totalPages={totalPages}
            pageSize={pageSize}
            currentLinkFactory={(index, pageSize) => {
                return `/accounts?page=${index - 1}&pageSize=${pageSize}`;
            }}
            nextLinkFactory={(curr, pageSize) => {
                return `/accounts?page=${curr + 1}&pageSize=${pageSize}`
            }}
            previousLinkFactory={(currentPage, pageSize) => {
                return `/accounts?page=${currentPage - 1}&pageSize=${pageSize}`
            }}
        />
    </section> : <></>;
}