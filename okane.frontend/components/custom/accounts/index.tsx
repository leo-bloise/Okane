import { Account } from "@/lib/types/accounts"
import AccountsPaginationHandler from "./AccountsPaginationHandler";
import AccountsTable from "./AccountsTable";

type Props = {
    accounts: Account[];
    currentPage: number;
    totalPages: number;
    pageSize: number;    
}

export default function AccountsSection({ accounts, currentPage, totalPages, pageSize }: Props) {
    return accounts.length > 0 ? <section className="flex flex-col gap-y-4">
        <AccountsTable accounts={accounts} />
        <AccountsPaginationHandler 
            currentPage={currentPage}
            totalPages={totalPages}
            pageSize={pageSize}
        />
    </section> : <></>;
}