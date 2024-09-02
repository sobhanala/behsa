export interface Transaction {
  TransactionId: number,
  sourceAccountId: number,
  destinationAccountId: number,
  amount: number,
  date: string,
  type: string
}

export interface Node {
  index?: number | undefined;
  x: number;
  y: number;
  vx: number;
  vy: number;
  fx?: number | null;
  fy?: number | null;
  label: string | number;
}

export interface Link {
  index?: number | undefined;
  source: Node;
  target: Node;
  date: string;
  amount: string;
  type: string;
}

export interface Account {
  "accountId": number,
  "cardId": number,
  "iban": string,
  "accountType": string,
  "branchTelephone": string,
  "branchAddress": string,
  "branchName": string,
  "ownerName": string,
  "ownerLastName": string,
  "ownerId": number
}

export interface AccountTransaction {
  transactionWithSources: {
    transactionID: number;
    sourceAcount: number;
    destiantionAccount: number;
    amount: number;
    date: string;
    type: string;
  }[];
  accountId: number;
}
