import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

type Props = {
    value: number;
    title: string;
    description: string;
    className?: string;
    red?: boolean;
    green?: boolean;
};

export default function CardIndicator({className = '', title, value, description, red = false, green = false}: Props) {
    const formatter = new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });

    return <Card className={className}>
        <CardHeader>
            <CardTitle>{title}</CardTitle>
            <CardDescription>{description}</CardDescription>
        </CardHeader>
        <CardContent>
            <p className={`text-xl ${green ? 'text-green-600' : red ? 'text-red-400' : ''}`}>
                {formatter.format(value)}
            </p>
        </CardContent>
    </Card>
}