import { ReactNode } from "react";
import { Link } from "react-router-dom";

interface PageHeaderProps {
    title: string;
    backTo?: string;
    backLabel?: string;
    badge?: ReactNode;
    actions?: ReactNode;
}

function PageHeader({ title, backTo, backLabel, badge, actions }: PageHeaderProps) {
    return (
        <div className="page-header">
            <div className="page-header-left">
                {backTo && (
                    <>
                        <Link to={backTo} className="back-link">
                            <i className="ti ti-arrow-left" /> {backLabel}
                        </Link>
                        <span className="crumb-sep">/</span>
                    </>
                )}
                <div className="page-header-title">{title}</div>
                {badge}
            </div>
            {actions && <div className="page-header-actions">{actions}</div>}
        </div>
    );
}

export default PageHeader;