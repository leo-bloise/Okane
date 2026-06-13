import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Account } from "@/lib/types/accounts";

export default function AccountsTable({ accounts }: { accounts: Account[] }) {
    return <Table>
        <TableHeader>
            <TableRow>
                <TableHead>Id</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Description</TableHead>
            </TableRow>
        </TableHeader>
        <TableBody>
            {accounts.map((account) => (
                <TableRow key={account.id}>
                    <TableCell>{account.id}</TableCell>
                    <TableCell>{account.name}</TableCell>
                    <TableCell>{account.description}</TableCell>
                </TableRow>
            ))}
        </TableBody>
    </Table>;
}