declare type Patrulje = {
    id: number
    name: string
    tenantId: string
}

declare type Point = {
    id: number
    points: number
    turnout: number
    tenantId: string
    patrulje: Patrulje
    post: Post
}

declare type Post = {
    id: number
    name: string
    tenantId: string
}

declare type Tenant = {
    id: string
    name: string
}

declare type TenantRole = 'SuperUser' | 'Administrator' | 'PostUser'

declare type TenantMember = {
    userId: string
    username: string
    tenantId: string
    roleName: TenantRole
}

declare type PointDTO = {
    point: number
    turnout: number
    patrulje: number
    post: number
}

declare type pointStats = {
    patruljeName: string
    totalPoints: number
    totalTurnout: number
    combinedTotal: number
}

declare type Me = {
    authenticated: boolean
    username: string | null
    isSuperUser: boolean
    currentTenantId: string | null
    role: TenantRole | null
    tenants: Tenant[]
}