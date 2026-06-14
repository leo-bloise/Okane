import { NextResponse } from "next/server";
import { Paged } from "../types/paged";

const getPageAndPageSize = (url: URL) => {
    const { searchParams } = url;

    let pageText = searchParams.get("page");
    let pageSizeText = searchParams.get("pageSize");

    if (!pageText) {
        pageText = '0';
    }

    if (!pageSizeText) {
        pageSizeText = '20';
    }

    let page = Number(pageText);
    let pageSize = Number(pageSizeText);

    if(Number.isNaN(page)) {
        page = 0;
    }

    if(Number.isNaN(pageSize)) {
        page = 20;
    }

    return {
        page,
        pageSize
    }
}


export const preparePagedGet = async <T>(sourceFunction: (page: number, pageSize: number) => Promise<Paged<T>>, request: Request) => {
    const url = new URL(request.url);

    const { page, pageSize } = getPageAndPageSize(url);

    const response = await sourceFunction(page, pageSize);

    return NextResponse.json(response, {
        status: 200
    });
}