'use client';

type PageHeaderProps = {
  title: string;
  description?: string;
  extra?: React.ReactNode;
};

export function PageHeader({ title, description, extra }: PageHeaderProps) {
  return (
    <div className="hc-proprietario-page-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: 16 }}>
      <div>
        <h1>{title}</h1>
        {description && <p>{description}</p>}
      </div>
      {extra}
    </div>
  );
}
