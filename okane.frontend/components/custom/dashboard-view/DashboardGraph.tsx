'use client';

import { DashboardReport } from "@/lib/types/dashboard-report";
import { Line, LineChart, Tooltip, XAxis, YAxis } from "recharts";

type Props = {
    report: DashboardReport
}

export default function DashboardGraph({ report }: Props) {
    const { balanceByDates } = report;
    const adjusted = balanceByDates.map(b => {
        return {
            ...b,
            timepoint: new Date(b.timepoint)
        }
    });
    const formatter = new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
    const formatterDate = new Intl.DateTimeFormat();
    console.log(adjusted);
    return <div className="w-full flex flex-col items-center justify-center h-full">
        <h2>Daily Balance</h2>
        <LineChart style={{ width: '100%', aspectRatio: 1.618, maxWidth: 600 }} responsive data={adjusted}>
            <XAxis width={'auto'} dataKey={'timepoint'} tickFormatter={(value: Date) => {
                return formatterDate.format(new Date(value));
            }} textAnchor="start" />
            <YAxis dataKey={'value'} width={'auto'} label={{
                value: 'Balance',
                position: 'insideLeft',
                angle: -90
            }} textAnchor="end" />
            <Line type={'monotone'} dataKey={'value'} stroke="black" strokeWidth={1.2} />
            <Line type={'monotone'} dataKey={'accumulatedBalance'} stroke="black" strokeWidth={1.2}></Line>
            <Tooltip
                formatter={(value, name) => {
                    if (typeof value !== 'number') return '';
                    const label = name === 'accumulatedBalance' ? 'Accumulated Value' : 'Balance';
                    return [formatter.format(value), label];
                }}
                labelFormatter={(label) => formatterDate.format(new Date(label))}
            />
        </LineChart>
    </div>
}