
graph TB
    C["🌐 Client Applications"]
    %% Address Service
    %% ========================
    subgraph AS["Address Service API"]
        ASA["API Layer"]
        ASApp["Application"]
        ASDom["Domain"]
        ASInf["Infrastructure"]
        ASTest["Tests"]

        ASA --> ASApp
        ASApp --> ASDom
        ASDom --> ASInf
        ASTest -.-> ASApp
        ASTest -.-> ASDom
    end

    %% ========================
    %% SurveyData Service
    %% ========================
    subgraph SS["SurveyData Service API"]
        SSA["API Layer"]
        SSApp["Application"]
        SSDom["Domain"]
        SSInf["Infrastructure"]
        SSTest["Tests"]

        SSA --> SSApp
        SSApp --> SSDom
        SSDom --> SSInf
        SSTest -.-> SSApp
        SSTest -.-> SSDom
    end

    DB["SQL Server Databases"]

    SPS["Shared Platform Services<br/>
    Auth • Logging • Observability<br/>
    Resilience • Health Checks • Tracing"]

    %% ========================
    %% Flows
    %% ========================
    C -->|HTTP/REST| ASA
    C -->|HTTP/REST| SSA

    ASInf --> DB
    SSInf --> DB

    ASA -.->|Uses| SPS
    SSA -.->|Uses| SPS
    ASApp -.->|Uses| SPS
    SSApp -.->|Uses| SPS

    %% ========================
    %% Styling
    %% ========================
    style C fill:#e1f5ff,stroke:#01579b,stroke-width:2px
    style AS fill:#fffde7,stroke:#f9a825,stroke-width:2px
    style SS fill:#fffde7,stroke:#f9a825,stroke-width:2px
    style DB fill:#f0f4c3,stroke:#558b2f,stroke-width:2px
    style SPS fill:#c8e6c9,stroke:#1b5e20,stroke-width:2px
