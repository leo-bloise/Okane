import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Transaction } from "@/lib/types/transaction";

export default function TransactionTable({ transactions }: { transactions: Transaction[] }) {
    return <Table>
        <TableHeader>
            <TableRow>
                <TableHead>Id</TableHead>
                <TableHead>Amount</TableHead>
                <TableHead>Description</TableHead>
                <TableHead>From Account</TableHead>
                <TableHead>To Account</TableHead>
                <TableHead>Occured At</TableHead>
            </TableRow>
        </TableHeader>
        <TableBody>
            {transactions.map((t) => (
                <TableRow key={t.id}>
                    <TableCell>{t.id}</TableCell>
                    <TableCell>{t.amount}</TableCell>
                    <TableCell>{t.description}</TableCell>
                    <TableCell>{t.fromAccountId}</TableCell>
                    <TableCell>{t.toAccountId}</TableCell>
                    <TableCell>{t.occured_at}</TableCell>
                </TableRow>
            ))}
        </TableBody>
    </Table>;
}