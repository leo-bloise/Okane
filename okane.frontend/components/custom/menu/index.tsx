import { ArrowRightLeft, CreditCard, LayoutDashboard } from "lucide-react";
import MenuView from "./MenuView";

export default function Menu() {
    const menuItems = [{
        icon: <LayoutDashboard size={16}/>,
        text: 'Dashboard',
        url: '/dashboard'
    }, {
        icon: <CreditCard min={16} size={16} />,
        text: 'Accounts',
        url: '/accounts'
    }, {
        icon: <ArrowRightLeft size={16} />,
        text: 'Transactions',
        url: '/transactions'
    }]

    return <MenuView
        items={menuItems}
    ></MenuView>
}